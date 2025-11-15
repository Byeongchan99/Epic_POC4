# 스테이지 커스터마이징 가이드 (프리팹 방식)

## 🎮 개요

Inspector에서 **각 스테이지에 사용할 MagicCircle 프리팹을 직접 할당**할 수 있습니다!
각 프리팹마다 다른 설정(패턴, 색상, 선 두께, 약점 비율 등)을 적용할 수 있어 훨씬 더 유연합니다.

## 📝 기본 설정 방법

### 1. GameManager 오브젝트 선택
- Hierarchy에서 **GameManager** 선택
- Inspector 창 확인

### 2. Stage Settings 섹션 찾기
```
Inspector > GameManager
└── Stage Settings
    └── Stage Prefabs (Array)
        ├── Size: 12 (스테이지 개수)
        ├── Element 0: [MagicCircle 프리팹을 드래그 앤 드롭]
        ├── Element 1: [MagicCircle 프리팹을 드래그 앤 드롭]
        ├── Element 2: [MagicCircle 프리팹을 드래그 앤 드롭]
        └── ...
```

### 3. 프리팹 할당하기
1. **Size** 값 설정 (원하는 스테이지 개수)
2. 각 **Element**에 프리팹 드래그 앤 드롭
   - Project 창에서 `Assets/Prefabs/MagicCircle.prefab` 찾기
   - Inspector의 Element 슬롯에 드래그 앤 드롭

## 🎨 프리팹별 커스터마이징

### 프리팹 만드는 법

#### 방법 1: 기존 프리팹 복제
1. Project 창에서 `Assets/Prefabs/MagicCircle.prefab` 우클릭
2. **Duplicate** 선택
3. 이름 변경 (예: `MagicCircle_Stage1.prefab`)
4. 프리팹 선택 > Inspector에서 설정 변경

#### 방법 2: 새로 만들기
1. Hierarchy에서 빈 GameObject 생성
2. **MagicCircle** 컴포넌트 추가
3. Inspector에서 원하는 값 설정
4. Project 창의 `Assets/Prefabs` 폴더로 드래그 앤 드롭

### 프리팹에서 설정 가능한 값들

각 프리팹마다 다르게 설정할 수 있는 항목:

```
🎨 Visual Settings
├── Pattern: 패턴 선택 (Circle, Triangle, Square, Pentagram 등)
├── Circle Color: 마법진 색상 (기본: 보라색)
├── Line Width: 선 두께 (기본: 0.1)
├── Broken Color: 끊어진 선분 색상 (기본: 회색)
├── Weakpoint Color: 약점 색상 (기본: 빨강)
└── Weakpoint Ratio: 약점 비율 (기본: 0.3 = 30%)

⚙️ Generation Settings
├── Radius: 마법진 크기 (기본: 2.0)
└── Draw Duration: 그려지는 시간 (GameManager에서 오버라이드됨)
```

## 🚀 실전 예시

### 예시 1: 단계별 난이도 증가 (12 스테이지)

#### 프리팹 12개 생성
1. `MagicCircle_Easy1.prefab` ~ `MagicCircle_Hard4.prefab` 생성

#### 각 프리팹 설정
```
Stage 1-3 (쉬움):
- Pattern: Triangle, Square, Pentagram
- Weakpoint Ratio: 0.2 (20%)
- Line Width: 0.12 (두껍게)
- Circle Color: 밝은 색 (0.5, 0.5, 1, 1)

Stage 4-6 (보통):
- Pattern: Hexagram, Heptagram, Octagram
- Weakpoint Ratio: 0.3 (30%)
- Line Width: 0.1
- Circle Color: 보통 색 (0.5, 0.2, 1, 1)

Stage 7-9 (어려움):
- Pattern: DoublePentagram, CrossPattern, Spiral
- Weakpoint Ratio: 0.4 (40%)
- Line Width: 0.08 (얇게)
- Circle Color: 어두운 색 (0.4, 0.1, 0.8, 1)

Stage 10-12 (매우 어려움):
- Pattern: InfinitySymbol, ComplexRune, Circle
- Weakpoint Ratio: 0.5 (50%)
- Line Width: 0.06 (매우 얇게)
- Circle Color: 거의 검정 (0.3, 0.1, 0.5, 1)
- Radius: 2.5 (더 크게)
```

#### GameManager 설정
```
Stage Settings > Stage Prefabs:
Size: 12
Element 0: MagicCircle_Easy1
Element 1: MagicCircle_Easy2
Element 2: MagicCircle_Easy3
Element 3: MagicCircle_Normal1
...
Element 11: MagicCircle_VeryHard4
```

### 예시 2: 색상 테마별 스테이지 (9 스테이지)

#### 프리팹 9개 생성
```
빨강 테마 (Stage 1-3):
- Circle Color: (1, 0.2, 0.2, 1)
- Weakpoint Color: (1, 1, 0.2, 1) - 노랑
- Pattern: Circle, Triangle, Square

초록 테마 (Stage 4-6):
- Circle Color: (0.2, 1, 0.2, 1)
- Weakpoint Color: (0.2, 0.2, 1, 1) - 파랑
- Pattern: Pentagram, Hexagram, Heptagram

파랑 테마 (Stage 7-9):
- Circle Color: (0.2, 0.2, 1, 1)
- Weakpoint Color: (1, 0.2, 1, 1) - 보라
- Pattern: Octagram, Spiral, ComplexRune
```

### 예시 3: 빠른 테스트 (3 스테이지)

#### 프리팹 3개만 생성
```
Stage 1: MagicCircle_Test_Easy
- Pattern: Triangle
- Weakpoint Ratio: 0.1 (매우 쉬움)

Stage 2: MagicCircle_Test_Normal
- Pattern: Pentagram
- Weakpoint Ratio: 0.3

Stage 3: MagicCircle_Test_Hard
- Pattern: Spiral
- Weakpoint Ratio: 0.5
```

#### GameManager 설정
```
Stage Settings > Stage Prefabs:
Size: 3
Element 0: MagicCircle_Test_Easy
Element 1: MagicCircle_Test_Normal
Element 2: MagicCircle_Test_Hard
```

### 예시 4: 같은 프리팹 반복 (특정 패턴 연습)

```
Stage Settings > Stage Prefabs:
Size: 5
Element 0-4: 모두 같은 프리팹 (MagicCircle_Pentagram)
```

같은 패턴을 5번 연속으로 플레이 (오각별 마스터 모드!)

## 🔧 고급 커스터마이징

### 난이도 조절 팁

#### 쉽게 만들기
1. **Weakpoint Ratio 감소**: `0.1` ~ `0.2`
2. **Line Width 증가**: `0.15` ~ `0.2` (찾기 쉬움)
3. **Circle Color 밝게**: `(0.7, 0.5, 1, 1)`
4. **적은 선분 패턴**: Triangle, Square, Pentagram
5. **Radius 감소**: `1.5` (작은 마법진)

#### 어렵게 만들기
1. **Weakpoint Ratio 증가**: `0.4` ~ `0.5`
2. **Line Width 감소**: `0.05` ~ `0.08` (찾기 어려움)
3. **Circle Color 어둡게**: `(0.3, 0.1, 0.5, 0.8)`
4. **많은 선분 패턴**: Spiral, InfinitySymbol, ComplexRune
5. **Radius 증가**: `3.0` (큰 마법진)

### 시각적 다양성

#### 무지개 스테이지
```
Stage 1: Circle Color (1, 0, 0, 1) - 빨강
Stage 2: Circle Color (1, 0.5, 0, 1) - 주황
Stage 3: Circle Color (1, 1, 0, 1) - 노랑
Stage 4: Circle Color (0, 1, 0, 1) - 초록
Stage 5: Circle Color (0, 0, 1, 1) - 파랑
Stage 6: Circle Color (0.5, 0, 1, 1) - 남색
Stage 7: Circle Color (1, 0, 1, 1) - 보라
```

#### 크기 변화 스테이지
```
Stage 1: Radius 1.0 (작음)
Stage 2: Radius 1.5
Stage 3: Radius 2.0 (보통)
Stage 4: Radius 2.5
Stage 5: Radius 3.0 (큼)
```

## 📋 프리팹 관리 팁

### 폴더 구조 추천
```
Assets/
└── Prefabs/
    ├── MagicCircle.prefab (기본)
    ├── Stages/
    │   ├── Easy/
    │   │   ├── MagicCircle_Easy1.prefab
    │   │   ├── MagicCircle_Easy2.prefab
    │   │   └── MagicCircle_Easy3.prefab
    │   ├── Normal/
    │   │   ├── MagicCircle_Normal1.prefab
    │   │   └── ...
    │   ├── Hard/
    │   │   └── ...
    │   └── VeryHard/
    │       └── ...
    └── Themed/
        ├── RedTheme_Stage1.prefab
        ├── GreenTheme_Stage1.prefab
        └── BlueTheme_Stage1.prefab
```

### 네이밍 컨벤션
```
MagicCircle_[난이도]_[스테이지번호].prefab
예: MagicCircle_Easy_01.prefab

또는

MagicCircle_[패턴이름]_[바리에이션].prefab
예: MagicCircle_Pentagram_Red.prefab
```

## 🎯 패턴별 추천 설정

### Circle (원형 - 32개 선분)
```
Weakpoint Ratio: 0.3-0.4 (약간 어려움)
Line Width: 0.08
난이도: 중상
```

### Triangle (삼각형 - 3개 선분)
```
Weakpoint Ratio: 0.2 (매우 쉬움)
Line Width: 0.15
난이도: 하
```

### Pentagram (오각별 - 5개 선분)
```
Weakpoint Ratio: 0.2-0.3
Line Width: 0.12
난이도: 중하
```

### Spiral (나선 - 64개 선분)
```
Weakpoint Ratio: 0.4-0.5 (어려움)
Line Width: 0.06
난이도: 상
```

### ComplexRune (복잡한 룬)
```
Weakpoint Ratio: 0.5 (매우 어려움)
Line Width: 0.05
난이도: 최상
```

## ⚠️ 주의사항

### 프리팭 수정 시 영향
- 프리팹을 수정하면 해당 프리팹을 사용하는 **모든 스테이지**에 영향
- 특정 스테이지만 바꾸려면 **프리팹을 복제**해서 사용

### 필수 체크리스트
- [ ] 모든 Element에 프리팹이 할당되어 있는가?
- [ ] 프리팹에 MagicCircle 컴포넌트가 있는가?
- [ ] 프리팹의 Pattern이 올바르게 설정되어 있는가?
- [ ] 색상이 배경과 구분되는가?
- [ ] Weakpoint Ratio가 0.1 ~ 0.5 범위인가?

### 디버깅
콘솔에 로그 확인:
```
"Stage X started with prefab: [프리팹 이름]"
"Stage X: 프리팹이 할당되지 않았습니다!" (에러)
```

## 🔄 기본 설정으로 돌아가기

### 옵션 1: 단일 프리팹 사용
```
Stage Settings > Stage Prefabs:
Size: 1
Element 0: MagicCircle (기본 프리팹)
```
→ 1스테이지만 무한 반복

### 옵션 2: Fallback 사용
```
Stage Settings > Stage Prefabs:
Size: 0 (비워두기)
```
→ GameManager의 magicCirclePrefab 사용 (1스테이지만)

## 💡 창의적 활용 예시

### 보스 스테이지
```
Stage 1-9: 일반 패턴 (쉬움~어려움)
Stage 10:
  - Pattern: ComplexRune
  - Radius: 3.5 (매우 크게)
  - Weakpoint Ratio: 0.5
  - Circle Color: (1, 0, 0, 1) - 빨강 (보스 느낌)
  - Line Width: 0.05
```

### 시간 제한 챌린지
```
GameManager > Cast Time: 2초 (짧게)
모든 프리팹 Weakpoint Ratio: 0.2 (쉽게)
→ 빠른 반응 속도 테스트
```

### 정확도 챌린지
```
GameManager > Cast Time: 10초 (길게)
모든 프리팹 Weakpoint Ratio: 0.5 (많게)
모든 프리팹 Line Width: 0.05 (얇게)
→ 정확한 조준 능력 테스트
```

## 📚 관련 문서

- [SETUP_GUIDE.md](SETUP_GUIDE.md) - 초기 세팅 방법
- [PATTERNS_GUIDE.md](PATTERNS_GUIDE.md) - 12가지 패턴 상세 설명
- [WEAKPOINT_SYSTEM.md](WEAKPOINT_SYSTEM.md) - 약점 시스템 설명
- [README.md](README.md) - 프로젝트 개요

---

이제 프리팹 방식으로 **무한한 커스터마이징**이 가능합니다! 🎉
